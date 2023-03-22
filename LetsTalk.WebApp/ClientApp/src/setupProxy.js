const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const apiTarget = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:6934';

const wsTarget = apiTarget;

const httpContext = ["/api", "/bff", "/signin-oidc", "/signout-callback-oidc"];
const wsContext = ["/hubs/letstalk"];

module.exports = function (app) {
    app.use(createProxyMiddleware(httpContext, {
        target: apiTarget,
        secure: false,
    }));
    app.use(createProxyMiddleware(wsContext, {
        target: wsTarget,
        ws: true,
        secure: false
    })
    );
};
