FROM node:20-alpine AS build
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable


ARG VITE_CLIENT_URL
ARG VITE_SERVER_URL

ENV VITE_CLIENT_URL=$VITE_CLIENT_URL
ENV VITE_SERVER_URL=$VITE_SERVER_URL

WORKDIR /app

COPY . .
RUN pnpm install
RUN pnpm run build

FROM nginx:alpine AS production

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]