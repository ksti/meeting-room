'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import styled from 'styled-components';
import bgImage from '@/assets/images/dragon-scales.svg';

export default function Home() {
  const router = useRouter();

  useEffect(() => {
    // 重定向到登录页面或仪表板
    const token = localStorage.getItem('token');
    if (token) {
      router.replace('/dashboard');
    } else {
      router.replace('/auth/login');
    }
  }, [router]);

  return (
    <StyledDiv className="bg-container min-h-screen p-8 pb-20 gap-16">
    </StyledDiv>
  );
}

const StyledDiv = styled.div`
  &.bg-container {
    background-image: url(${bgImage.src});
    background-size: contain;
    background-position: center;
    background-repeat: repeat;
  }
`;
