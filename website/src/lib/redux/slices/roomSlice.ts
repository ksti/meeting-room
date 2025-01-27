import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '../../api/axios';

export interface Room {
  id: string;
  name: string;
  capacity: number;
  facilities: string[];
  isAvailable: boolean;
}

interface RoomState {
  rooms: Room[];
  loading: boolean;
  error: string | null;
  selectedRoom: Room | null;
}

const initialState: RoomState = {
  rooms: [],
  loading: false,
  error: null,
  selectedRoom: null,
};

export const fetchRooms = createAsyncThunk('rooms/fetchRooms', async (_, { rejectWithValue }) => {
  try {
    const response = await api.get('/api/rooms');
    return response.data;
  } catch (error: any) {
    return rejectWithValue(error.response?.data?.message || 'Failed to fetch rooms');
  }
});

export const createRoom = createAsyncThunk(
  'rooms/createRoom',
  async (roomData: Omit<Room, 'id'>, { rejectWithValue }) => {
    try {
      const response = await api.post('/api/rooms', roomData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create room');
    }
  }
);

export const updateRoom = createAsyncThunk(
  'rooms/updateRoom',
  async ({ id, data }: { id: string; data: Partial<Room> }, { rejectWithValue }) => {
    try {
      const response = await api.put(`/api/rooms/${id}`, data);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update room');
    }
  }
);

const roomSlice = createSlice({
  name: 'rooms',
  initialState,
  reducers: {
    selectRoom: (state, action) => {
      state.selectedRoom = action.payload;
    },
    clearRoomError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchRooms.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchRooms.fulfilled, (state, action) => {
        state.loading = false;
        state.rooms = action.payload;
      })
      .addCase(fetchRooms.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(createRoom.fulfilled, (state, action) => {
        state.rooms.push(action.payload);
      })
      .addCase(updateRoom.fulfilled, (state, action) => {
        const index = state.rooms.findIndex((room) => room.id === action.payload.id);
        if (index !== -1) {
          state.rooms[index] = action.payload;
        }
      });
  },
});

export const { selectRoom, clearRoomError } = roomSlice.actions;
export default roomSlice.reducer;
