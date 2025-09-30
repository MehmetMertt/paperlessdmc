# syntax=docker/dockerfile:1

##############################
# 1) Build Stage mit Node
##############################
FROM node:22-alpine AS builder

WORKDIR /app

# Copy package files first for caching
COPY package*.json ./

# Install dependencies
RUN npm install


# Copy the rest of the source
COPY . .

# Build Angular app
RUN npm run build -- --configuration production
#RUN npx ngcc --properties es2023 browser module main --first-only --create-ivy-entry-points
##############################
# 2) Nginx Stage für Deployment
##############################
FROM nginx:stable-alpine

# Replace default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf
COPY defaultNginx.conf /etc/nginx/conf.d/default.conf

# Copy Angular build output
COPY --from=builder /app/dist/file-managment-gui/browser /usr/share/nginx/html

# Healthcheck
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -qO- http://localhost:80 || exit 1

EXPOSE 80
