/** @type {import('next').NextConfig} */

const config = require('./src/constants/base-url');

const nextConfig = {
  basePath: config.basePath,
  reactStrictMode: true,
  images: {
    unoptimized: true
  },
  transpilePackages: ["@mui/x-charts"],
};

module.exports = nextConfig;