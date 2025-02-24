import React from 'react';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  TextField,
  Button,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Autocomplete,
} from '@mui/material';
import dayjs, { Dayjs } from 'dayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/lib/redux/store';
import { createMeeting, updateMeeting } from '@/lib/redux/slices/meetingSlice';
import { Room } from '@/lib/redux/slices/roomSlice';
import { User } from '@/lib/redux/slices/userSlice';

const schema = z.object({
  title: z.string({ required_error: 'Title is required' }),
  description: z.string({ required_error: 'Description is required' }),
  roomId: z.string({ required_error: 'Room is required' }),
  startTime: z.date({ required_error: 'Start time is required' }),
  endTime: z.date({ required_error: 'End time is required' }),
  attendees: z.array(z.object({
    id: z.string({ required_error: 'User id is required' }),
    usermane: z.string({ required_error: 'Username is required' }),
  })).min(1, 'At least one attendee is required'),
}).refine((data) => data.endTime > data.startTime, {
  message: "End time must be after start time.",
  path: ["endTime"],
});

interface MeetingFormData {
  title: string;
  description: string;
  roomId: string;
  startTime: Dayjs;
  endTime: Dayjs;
  attendees: User[];
}

interface MeetingFormProps {
  initialData?: MeetingFormData;
  onSuccess?: () => void;
}

export default function MeetingForm({ initialData, onSuccess }: MeetingFormProps) {
  const dispatch = useDispatch<AppDispatch>();
  const rooms = useSelector((state: RootState) => state.rooms.rooms);
  const users = useSelector((state: RootState) => state.users.users);
  const currentUser = useSelector((state: RootState) => state.auth.user);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
  } = useForm<MeetingFormData>({
    resolver: zodResolver(schema),
    defaultValues: initialData,
  });

  const onSubmit = async (data: MeetingFormData) => {
    try {
      if (initialData) {
        await dispatch(
          updateMeeting({
            id: (initialData as any).id,
            data: {
              ...data,
              startTime: data.startTime.toISOString(),
              endTime: data.endTime.toISOString(),
              organizer: currentUser,
            },
          })
        ).unwrap();
      } else {
        await dispatch(
          createMeeting({
            ...data,
            startTime: data.startTime.toISOString(),
            endTime: data.endTime.toISOString(),
            organizer: currentUser,
          })
        ).unwrap();
      }
      onSuccess?.();
    } catch (error) {
      console.error('Meeting operation failed:', error);
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 1 }}>
      <TextField
        margin="normal"
        required
        fullWidth
        id="title"
        label="Meeting Title"
        {...register('title')}
        error={!!errors.title}
        helperText={errors.title?.message}
      />

      <TextField
        margin="normal"
        required
        fullWidth
        id="description"
        label="Description"
        multiline
        rows={4}
        {...register('description')}
        error={!!errors.description}
        helperText={errors.description?.message}
      />

      <FormControl fullWidth margin="normal">
        <InputLabel id="room-label">Room</InputLabel>
        <Controller
          name="roomId"
          control={control}
          render={({ field }) => (
            <Select
              labelId="room-label"
              label="Room"
              error={!!errors.roomId}
              {...field}
            >
              {rooms.map((room: Room) => (
                <MenuItem key={room.id} value={room.id}>
                  {room.name} (Capacity: {room.capacity})
                </MenuItem>
              ))}
            </Select>
          )}
        />
      </FormControl>

      <Controller
        name="startTime"
        control={control}
        render={({ field }) => (
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DateTimePicker
              label="Start Time"
              {...field}
              sx={{ mt: 2, width: '100%' }}
            />
          </LocalizationProvider>
        )}
      />

      <Controller
        name="endTime"
        control={control}
        render={({ field }) => (
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DateTimePicker
              label="End Time"
              {...field}
              sx={{ mt: 2, width: '100%' }}
            />
          </LocalizationProvider>
        )}
      />

      <Controller
        name="attendees"
        control={control}
        render={({ field: { onChange, value } }) => (
          <Autocomplete
            multiple
            options={users.filter((user: User) => user.id !== currentUser.id)}
            getOptionLabel={(option: User) => `${option.username} (${option.email})`}
            value={users.filter((user: User) => value && value.findIndex(u => u.id === user.id) != -1) || []}
            onChange={(_, newValue) => {
              // onChange(newValue.map((user: User) => user.id));
              onChange(newValue);
            }}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Attendees"
                margin="normal"
                error={!!errors.attendees}
                helperText={errors.attendees?.message}
              />
            )}
          />
        )}
      />

      <Button type="submit" fullWidth variant="contained" sx={{ mt: 3, mb: 2 }}>
        {initialData ? 'Update Meeting' : 'Create Meeting'}
      </Button>
    </Box>
  );
}
