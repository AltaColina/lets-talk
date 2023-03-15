const Docker = require("dockerode");
const fs = require('fs');

var docker = new Docker();

const containers = docker
  .listContainers()
  .then(containers => {
    const webApiName = '/LetsTalk.WebApi';
    const webApi = containers
      .find(container => container.Names.indexOf(webApiName) >= 0);
    if (!webApi)
      throw new Error(`Did not find container '${webApiName}'`);
    const webApiPort = webApi.Ports.find(p => p.PrivatePort === 443);
    const package = JSON.parse(fs.readFileSync('package.json', { encoding: 'utf8' }));
    package.proxy = `https://localhost:${webApiPort.PublicPort}`;
    fs.writeFileSync('package.json', JSON.stringify(package, undefined, 2), { encoding: 'utf-8' });
  });
