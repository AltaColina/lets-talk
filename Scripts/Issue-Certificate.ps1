# Source: https://stackoverflow.com/a/62060315
# Generate self-signed certificate to be used by LetsTalk.Identity.
# When using localhost - LetsTalk.WebApi cannot see LetsTalk.Identity from within the docker-compose'd network.
# You have to run this script as Administrator (open Powershell by right click -> Run as Administrator).

$ErrorActionPreference = "Stop"

$rootCN = "LetsTalkRootCert"
$identityCNs = "identity", "localhost"
$webApiCNs = "webapi", "localhost"

# Comment the next 3 lines to ovewrite.
$alreadyExistingCertsRoot = Get-ChildItem -Path Cert:\LocalMachine\My -Recurse | Where-Object {$_.Subject -eq "CN=$rootCN"}
$alreadyExistingCertsIdentity = Get-ChildItem -Path Cert:\LocalMachine\My -Recurse | Where-Object {$_.Subject -eq ("CN={0}" -f $identityCNs[0])}
$alreadyExistingCertsWebApi = Get-ChildItem -Path Cert:\LocalMachine\My -Recurse | Where-Object {$_.Subject -eq ("CN={0}" -f $webApiCNs[0])}

if ($alreadyExistingCertsRoot.Count -eq 1) {
    Write-Output "Skipping creating Root CA certificate as it already exists."
    $rootCA = [Microsoft.CertificateServices.Commands.Certificate] $alreadyExistingCertsRoot[0]
} else {
    $rootCA = New-SelfSignedCertificate -Subject $rootCN -KeyUsageProperty Sign -KeyUsage CertSign -CertStoreLocation Cert:\LocalMachine\My
}

if ($alreadyExistingCertsIdentity.Count -eq 1) {
    Write-Output "Skipping creating Identity certificate as it already exists."
    $identityCert = [Microsoft.CertificateServices.Commands.Certificate] $alreadyExistingCertsIdentity[0]
} else {
    # Create a SAN cert for both identity and localhost.
    $identityCert = New-SelfSignedCertificate -DnsName $identityCNs -Signer $rootCA -CertStoreLocation Cert:\LocalMachine\My
}

if ($alreadyExistingCertsWebApi.Count -eq 1) {
    Write-Output "Skipping creating WebApi certificate as it already exists."
    $webApiCert = [Microsoft.CertificateServices.Commands.Certificate] $alreadyExistingCertsWebApi[0]
} else {
    # Create a SAN cert for both webapi and localhost.
    $webApiCert = New-SelfSignedCertificate -DnsName $webApiCNs -Signer $rootCA -CertStoreLocation Cert:\LocalMachine\My
}

$projPath = New-Object System.IO.DirectoryInfo $PSScriptRoot;
$projPath = $projPath.Parent.FullName;

# Export it for docker container to pick up later.
$password = ConvertTo-SecureString -String "password" -Force -AsPlainText

$certPathPfx = "$projPath/Certificates"

[System.IO.Directory]::CreateDirectory($certPathPfx) | Out-Null

Export-PfxCertificate -Cert $rootCA -FilePath "$certPathPfx/root-cert.pfx" -Password $password | Out-Null
Export-PfxCertificate -Cert $identityCert -FilePath "$certPathPfx/identity.pfx" -Password $password | Out-Null
Export-PfxCertificate -Cert $webApiCert -FilePath "$certPathPfx/webapi.pfx" -Password $password | Out-Null

# Export .cer to be converted to .crt to be trusted within the Docker container.
$rootCertPathCer = "$certPathPfx/root-cert.cer"
Export-Certificate -Cert $rootCA -FilePath $rootCertPathCer -Type CERT | Out-Null

# Trust it on your host machine.
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root","LocalMachine"
$store.Open("ReadWrite")

$rootCertAlreadyTrusted = ($store.Certificates | Where-Object {$_.Subject -eq "CN=$rootCN"} | Measure-Object).Count -eq 1

if ($rootCertAlreadyTrusted -eq $false) {
    Write-Output "Adding the root CA certificate to the trust store."
    $store.Add($rootCA)
}

$store.Close()