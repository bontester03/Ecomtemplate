FROM node:22-alpine AS build
ARG API_URL=http://localhost:5001/api
WORKDIR /app
COPY ./web/package*.json ./
RUN npm ci
COPY ./web .
RUN npm run build
RUN find /app/dist -type f -name '*.js' -exec sed -i "s|__API_BASE_URL__|${API_URL}|g" {} +

FROM nginx:1.27-alpine AS final
COPY --from=build /app/dist/web/browser /usr/share/nginx/html
COPY ./web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
