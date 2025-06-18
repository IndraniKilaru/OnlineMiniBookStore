# Build stage
FROM node:18-alpine as build

# Set working directory
WORKDIR /app

# Copy package files for better layer caching
COPY package*.json ./

# Install dependencies (including dev dependencies for build)
RUN npm install

# Copy source code
COPY . .

# Build the Angular app for production
RUN npm run build -- --configuration production

# Production stage
FROM nginx:alpine

# Install security updates
RUN apk --no-cache add ca-certificates && \
    apk --no-cache update

# Use existing nginx user from nginx:alpine image

# Copy built app from build stage
COPY --from=build /app/dist/bookstore-frontend /usr/share/nginx/html

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
