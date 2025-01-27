/**
 * 基础路径配置
 * 注意：这里使用环境变量，确保在 .env.development 和 .env.production 中正确设置
 * NEXT_PUBLIC_BASE_PATH 的值
 */
export const BASE_PATH = process.env.NEXT_PUBLIC_BASE_PATH || '';

/**
 * 获取资源路径
 * @param path - 相对路径
 * @returns 完整的资源路径
 */
export const getAssetPath = (path: string): string => {
  // 如果是外部 URL，直接返回
  if (path.startsWith('http')) {
    return path;
  }
  
  // 确保路径以 / 开头
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  const basePath = process.env.NEXT_PUBLIC_BASE_PATH || '';
  return `${basePath}${normalizedPath}`;
};

/**
 * 获取 CSS 变量声明
 * 用于在 layout.tsx 中设置全局 CSS 变量
 */
export const getCssVariables = () => {
  const basePath = process.env.NEXT_PUBLIC_BASE_PATH || '';
  return `
    :root {
      --base-path: '${basePath}';
      --globe-bg-url: url('${basePath}/globe.svg');
    }
  `;
};

// 获取API路径
export const getApiPath = (path: string): string => {
  const basePath = process.env.NEXT_PUBLIC_BASE_PATH || '';
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  return `${basePath}/api${normalizedPath}`;
};
