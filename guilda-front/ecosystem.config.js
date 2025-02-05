module.exports = {
  apps : [{
    name: "front",
    script: "node_modules/next/dist/bin/next",
    args: "start",
    instances: 1,
    watch: false,
    env: {
      NODE_ENV: "production",
    },
  }],
};