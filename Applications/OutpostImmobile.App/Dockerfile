# Step 1: Use a modern Node.js image to build the app
FROM node:22-alpine AS builder

WORKDIR /app

# Copy package files (Assumes build context is the Repo Root)
COPY Applications/OutpostImmobile.App/package*.json ./

RUN npm install

# Copy the source code
COPY Applications/OutpostImmobile.App/ ./

RUN npm run build

# Step 2: Use an Nginx image to serve the static files
FROM nginx:alpine

# Copy the build output (Ensure your project uses Vite. If using CRA, change 'dist' to 'build')
COPY --from=builder /app/dist /usr/share/nginx/html

# FIX: Configure Nginx to listen on 5002 instead of default 80
RUN sed -i 's/80/5002/g' /etc/nginx/conf.d/default.conf

EXPOSE 5002

CMD ["nginx", "-g", "daemon off;"]