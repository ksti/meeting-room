/** @type {import('next').NextConfig} */
const nextConfig = {
  // 设置基础路径
  basePath: process.env.NEXT_PUBLIC_BASE_PATH || '',
  
  // 资源前缀
  assetPrefix: process.env.NEXT_PUBLIC_ASSET_PREFIX || '',
  
  // 输出配置 - standalone 模式适合容器化部署
  output: 'standalone',
  
  // 样式配置
  compiler: {
    styledComponents: true,
  },

  // 图片配置
  images: {
    loader: 'custom',
    loaderFile: './src/lib/utils/imageLoader.ts',
    unoptimized: true,
  },

  // Webpack 配置
  webpack: (config) => {
    // 配置 ~public 前缀
    config.resolve.alias = {
      ...config.resolve.alias,
      '~public': '.',  // 将 ~public 映射到项目根目录
    };

    return config;
  },
}

export default nextConfig;
