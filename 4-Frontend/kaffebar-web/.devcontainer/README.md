# Devcontainer med Podman

Åpne mappen i VS Code med Dev Containers-utvidelsen. Velg Podman som container CLI
i VS Code-innstillingen `Dev Containers: Docker Path` (verdien er `podman`).

Devcontaineren starter RabbitMQ og installerer Yarn-avhengighetene automatisk.
Start utviklingsserveren med `yarn dev` i containeren.

Appen bruker mock-API som standard. For et eksternt API, sett `KAFFEBAR_API_URL` til
en adresse som er tilgjengelig fra Podman-maskinen.