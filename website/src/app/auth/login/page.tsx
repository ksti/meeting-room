'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Container, Paper, Typography, Box } from '@mui/material';
import styled from 'styled-components';
import LoginForm from '@/components/forms/LoginForm';
import { useSelector } from 'react-redux';
import { RootState } from '@/lib/redux/store';

export default function LoginPage() {
  const router = useRouter();
  const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

  useEffect(() => {
    if (isAuthenticated) {
      router.push('/dashboard');
    }
  }, [isAuthenticated, router]);

  return (
    <StyledDiv className='bg-container min-h-screen'>
      <StyledContainer component="main" maxWidth="xs" className='bg-login'>
        <Box
          sx={{
            paddingTop: 8,
            paddingBottom: 8,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          <Paper elevation={3} sx={{ p: 4, width: '100%' }}>
            <Typography component="h1" variant="h5" align="center" gutterBottom>
              Meeting Room Reservation
            </Typography>
            <Typography component="h2" variant="h6" align="center" gutterBottom>
              Sign in
            </Typography>
            <LoginForm />
          </Paper>
        </Box>
      </StyledContainer>
    </StyledDiv>
  );
}

const StyledContainer = styled(Container)`
  &.bg-login {
    background-image: var(--bg-url-prism);
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
  }
` as typeof Container;

const StyledDiv = styled.div`
  &.bg-container {
    background-image: url(${require('@/assets/images/dragon-scales.svg').default.src});
    background-size: contain;
    background-position: center;
    background-repeat: repeat;
  }
`;
