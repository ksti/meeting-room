import { ReduxProvider } from '@/lib/providers/ReduxProvider';
import { getCssVariables } from '@/lib/utils/paths';
import '@/styles/globals.css';

export const metadata = {
  title: 'Meeting Room',
  description: 'Meeting room reservation system',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  // 获取 CSS 变量样式
  const cssVariables = getCssVariables();

  return (
    <html lang="en">
      <head>
        {/* 使用 Next.js Script 组件添加内联样式 */}
        <script
          dangerouslySetInnerHTML={{
            __html: `
              (function() {
                const style = document.createElement('style');
                style.textContent = ${JSON.stringify(cssVariables)};
                document.head.appendChild(style);
              })();
            `,
          }}
        />
      </head>
      <body>
        <ReduxProvider>
          {children}
        </ReduxProvider>
      </body>
    </html>
  );
}
