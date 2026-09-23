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
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
