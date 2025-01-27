import { getAssetPath } from './paths';

/**
 * Next.js 图片加载器
 * 用于处理图片资源的路径
 */
export default function imageLoader({ src, width, quality }: { src: string; width: number; quality?: number }) {
  return getAssetPath(src);
}
