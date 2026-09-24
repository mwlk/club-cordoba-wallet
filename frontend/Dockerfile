# Build
FROM node:22-alpine AS build
WORKDIR /app
RUN corepack enable
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml .
RUN pnpm install --frozen-lockfile
COPY . .
RUN pnpm run build

# Serve
FROM nginx:alpine
COPY --from=build /app/dist/club-cordoba-wallet-ui/browser /usr/share/nginx/html
# En /etc/nginx/templates/, la imagen oficial corre envsubst sobre este
# archivo al arrancar y lo deja en conf.d/ con las variables ya resueltas
# (default acá por si se corre la imagen suelta, sin compose).
COPY nginx.conf.template /etc/nginx/templates/default.conf.template
ENV UI_INTERNAL_PORT=80
ENV API_INTERNAL_PORT=8080
EXPOSE 80
