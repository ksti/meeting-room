'use client';

import { useEffect, useState } from 'react';
import {
  Grid,
  Paper,
  Typography,
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { useDispatch, useSelector } from 'react-redux';
import dayjs, { Dayjs } from 'dayjs';
import { AppDispatch, RootState } from '@/lib/redux/store';
import ScheduleCalendar from '@/components/ui/ScheduleCalendar';
import MeetingForm from '@/components/forms/MeetingForm';
import DashboardLayout from '@/components/layout/DashboardLayout';
import { fetchMeetings, Meeting } from '@/lib/redux/slices/meetingSlice';
import { fetchRooms } from '@/lib/redux/slices/roomSlice';
import { fetchUsers } from '@/lib/redux/slices/userSlice';

export default function DashboardPage() {
  const dispatch = useDispatch<AppDispatch>();
  const [selectedMeeting, setSelectedMeeting] = useState<Meeting>();
  const [isCreateMeetingOpen, setIsCreateMeetingOpen] = useState(false);
  const meetings = useSelector((state: RootState) => state.meetings.meetings);

  useEffect(() => {
    dispatch(fetchMeetings());
    dispatch(fetchRooms());
    dispatch(fetchUsers());
  }, [dispatch]);

  const handleMeetingSelect = (meetingId: string) => {
    setSelectedMeeting(meetings.find((meeting) => meeting.id === meetingId));
    setIsCreateMeetingOpen(true);
  };

  const handleCreateMeetingClick = () => {
    setSelectedMeeting(undefined);
    setIsCreateMeetingOpen(true);
  };

  const handleCreateMeetingClose = () => {
    setIsCreateMeetingOpen(false);
  };

  const handleMeetingSuccess = () => {
    setIsCreateMeetingOpen(false);
    dispatch(fetchMeetings());
  };

  return (
    <DashboardLayout>
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
              <Typography component="h2" variant="h6" color="primary" gutterBottom>
                Calendar
              </Typography>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={handleCreateMeetingClick}
              >
                New Meeting
              </Button>
            </Box>
            <ScheduleCalendar mettings={meetings} onMeetingSelect={handleMeetingSelect} />
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
            <Typography component="h2" variant="h6" color="primary" gutterBottom>
              Today's Meetings
            </Typography>
            {/* Add meetings list component here */}
          </Paper>
        </Grid>
      </Grid>

      <Dialog
        open={isCreateMeetingOpen}
        onClose={handleCreateMeetingClose}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>{selectedMeeting ? 'Update Meeting' : 'Schedule New Meeting'}</DialogTitle>
        <DialogContent>
          <MeetingForm
            initialData={!selectedMeeting ? undefined : {
              ...selectedMeeting,
              startTime: dayjs(selectedMeeting?.startTime),
              endTime: dayjs(selectedMeeting?.endTime),
            }}
            onSuccess={handleMeetingSuccess}
          />
        </DialogContent>
      </Dialog>
    </DashboardLayout>
  );
}
