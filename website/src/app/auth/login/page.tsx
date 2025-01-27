'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Container, Paper, Typography, Box } from '@mui/material';
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
    <Container component="main" maxWidth="xs" className='bg-globe'>
      <Box
        sx={{
          marginTop: 8,
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
    </Container>
  );
}
