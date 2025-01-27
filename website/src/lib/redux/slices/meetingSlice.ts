import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { api } from '../../api/axios';

export interface Meeting {
  id: string;
  title: string;
  description: string;
  roomId: string;
  startTime: string;
  endTime: string;
  organizer: string;
  attendees: string[];
  status: 'scheduled' | 'ongoing' | 'completed' | 'cancelled';
}

interface MeetingState {
  meetings: Meeting[];
  userMeetings: Meeting[];
  loading: boolean;
  error: string | null;
  selectedMeeting: Meeting | null;
}

const initialState: MeetingState = {
  meetings: [],
  userMeetings: [],
  loading: false,
  error: null,
  selectedMeeting: null,
};

export const fetchMeetings = createAsyncThunk(
  'meetings/fetchMeetings',
  async (_, { rejectWithValue }) => {
    try {
      const response = await api.get('/api/meetings');
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch meetings');
    }
  }
);

export const fetchUserMeetings = createAsyncThunk(
  'meetings/fetchUserMeetings',
  async (userId: string, { rejectWithValue }) => {
    try {
      const response = await api.get(`/api/meetings/user/${userId}`);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to fetch user meetings');
    }
  }
);

export const createMeeting = createAsyncThunk(
  'meetings/createMeeting',
  async (meetingData: Omit<Meeting, 'id' | 'status'>, { rejectWithValue }) => {
    try {
      const response = await api.post('/api/meetings', meetingData);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create meeting');
    }
  }
);

export const updateMeeting = createAsyncThunk(
  'meetings/updateMeeting',
  async ({ id, data }: { id: string; data: Partial<Meeting> }, { rejectWithValue }) => {
    try {
      const response = await api.put(`/api/meetings/${id}`, data);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update meeting');
    }
  }
);

export const cancelMeeting = createAsyncThunk(
  'meetings/cancelMeeting',
  async (id: string, { rejectWithValue }) => {
    try {
      const response = await api.put(`/api/meetings/${id}/cancel`);
      return response.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to cancel meeting');
    }
  }
);

const meetingSlice = createSlice({
  name: 'meetings',
  initialState,
  reducers: {
    selectMeeting: (state, action) => {
      state.selectedMeeting = action.payload;
    },
    clearMeetingError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchMeetings.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchMeetings.fulfilled, (state, action) => {
        state.loading = false;
        state.meetings = action.payload;
      })
      .addCase(fetchMeetings.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchUserMeetings.fulfilled, (state, action) => {
        state.userMeetings = action.payload;
      })
      .addCase(createMeeting.fulfilled, (state, action) => {
        state.meetings.push(action.payload);
      })
      .addCase(updateMeeting.fulfilled, (state, action) => {
        const index = state.meetings.findIndex((meeting) => meeting.id === action.payload.id);
        if (index !== -1) {
          state.meetings[index] = action.payload;
        }
      })
      .addCase(cancelMeeting.fulfilled, (state, action) => {
        const index = state.meetings.findIndex((meeting) => meeting.id === action.payload.id);
        if (index !== -1) {
          state.meetings[index] = action.payload;
        }
      });
  },
});

export const { selectMeeting, clearMeetingError } = meetingSlice.actions;
export default meetingSlice.reducer;
