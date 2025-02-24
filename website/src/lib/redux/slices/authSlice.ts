import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '../../api/axios';

const useMockData = process.env.NEXT_PUBLIC_USE_MOCK_DATA === 'true';

interface AuthState {
  user: any | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  user: typeof window !== 'undefined' ? JSON.parse(localStorage.getItem('user') ?? '{}') : null,
  accessToken: typeof window !== 'undefined' ? localStorage.getItem('accessToken') : null,
  isAuthenticated: false,
  loading: false,
  error: null,
};

export const login = createAsyncThunk(
  'auth/login',
  async (credentials: { email: string; password: string }, { rejectWithValue }) => {
    try {
      if (useMockData) {
        const mockResponse = {
          user: {
            id: '1',
            email: 'user1@example.com',
            username: 'user1',
          },
          accessToken: 'mock-accessToken',
        };
        localStorage.setItem('accessToken', mockResponse.accessToken);
        localStorage.setItem('user', JSON.stringify(mockResponse.user));
        return mockResponse;
      }
      const response = await api.post('/api/auth/login', credentials);
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('user', response.data.user);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Login failed');
    }
  }
);

export const logout = createAsyncThunk('/api/auth/logout', async () => {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('user');
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        state.isAuthenticated = true;
        state.user = action.payload.user;
        state.accessToken = action.payload.accessToken;
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(logout.fulfilled, (state) => {
        state.user = null;
        state.accessToken = null;
        state.isAuthenticated = false;
      });
  },
});

export const { clearError } = authSlice.actions;
export default authSlice.reducer;
