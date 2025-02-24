import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '../../api/axios';

const useMockData = process.env.NEXT_PUBLIC_USE_MOCK_DATA === 'true';

export interface User {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  status: 'active' | 'disabled';
  role: 'admin' | 'user';
  department?: string;
}

interface UserState {
  users: User[];
  loading: boolean;
  error: string | null;
  selectedUser: User | null;
}

const initialState: UserState = {
  users: [],
  loading: false,
  error: null,
  selectedUser: null,
};

export const fetchUsers = createAsyncThunk('users/fetchUsers', async (_, { rejectWithValue }) => {
  try {
    if (useMockData) {
      return [
        {
          id: '1',
          username: 'user1',
          firstName: 'John',
          lastName: 'Doe',
          email: 'user1@example.com',
          role: 'admin',
          status: 'active',
        },
        {
          id: '2',
          username: 'user2',
          firstName: 'Alice',
          lastName: 'Doe',
          email: 'user2@example.com',
          role: 'user',
          status: 'active',
        },
        {
          id: '3',
          username: 'user3',
          firstName: 'John',
          lastName: 'Doe',
          email: 'user3@example.com',
          role: 'user',
          status: 'disabled',
        },
      ];
    }
    const response = await api.get('/api/users');
    return response.data;
  } catch (error: any) {
    return rejectWithValue(error.response?.data?.message || 'Failed to fetch users');
  }
});

export const createUser = createAsyncThunk(
  'users/createUser',
  async (userData: Omit<User, 'id'>, { rejectWithValue }) => {
    try {
      const response = await api.post('/api/users', userData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create user');
    }
  }
);

export const updateUser = createAsyncThunk(
  'users/updateUser',
  async ({ id, data }: { id: string; data: Partial<User> }, { rejectWithValue }) => {
    try {
      const response = await api.put(`/api/users/${id}`, data);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update user');
    }
  }
);

const userSlice = createSlice({
  name: 'users',
  initialState,
  reducers: {
    selectUser: (state, action) => {
      state.selectedUser = action.payload;
    },
    clearUserError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.loading = false;
        state.users = action.payload;
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(createUser.fulfilled, (state, action) => {
        state.users.push(action.payload);
      })
      .addCase(updateUser.fulfilled, (state, action) => {
        const index = state.users.findIndex((user) => user.id === action.payload.id);
        if (index !== -1) {
          state.users[index] = action.payload;
        }
      });
  },
});

export const { selectUser, clearUserError } = userSlice.actions;
export default userSlice.reducer;
