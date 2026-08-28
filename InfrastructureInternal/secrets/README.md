# Docker secrets


## Secret files in docker compose
- Docker secret files in DEV with docker compose are plain text files.
- Files in this folder are sample files and the content can be adjusted to each developers needs.
- For easier development some services have their login completely disabled.

 
## Secret files in docker stack (swarm)
- Docker secrets in QAT and PRD are created via the pipelines by using `docker secret create`.
- Those secrets are encrypted at rest and get mounted into docker container into a temporary memory.

 
## Docker compose vs docker stack yaml configuration
In docker compose we reference the "secret" files via the following lines:
```
secrets:
  mysecret:
    file: ./mysecretfile
```

In docker stack we reference the secrets via the following lines:
```
secrets:
  mysecret:
    external: true
```

One can create the secrets by using one of the following commands:
```
docker secret create mysecret ./mysecretfile.sec
echo "mypassword" | docker secret create mysecret -
echo "mykey=myvalue" | docker secret create mysecret -
echo "{ 'mykey':'myvalue' }" | docker secret create mysecret -
```


## Hardening Docker-Desktop
For people whom don't want their exposed docker desktops service accessible form outside their machine can configure their firewall.
When docker desktop gets installed it also creates some firewall rules that allow your exposed containers to be accessible from the outside.
 
| Remote | Local Docker Desktop |
|--------|----------------------|
| http://172.20.21.22:42003 | localhost:42003  (exposed port) |
 
This might be a security concern to some people but can be easily disabled by doing the following:
- Open Windows Defender Firewall
- Go to Inbound Rules
- Select Docker Desktop Backend UDP and right click: Disable Rule
- Select Docker Desktop Backend TCP and right click: Disable Rule 



## Resources
- [How to use secrets](https://docs.docker.com/compose/how-tos/use-secrets)
- [Docker Secret Doc](https://docs.docker.com/reference/cli/docker/secret)