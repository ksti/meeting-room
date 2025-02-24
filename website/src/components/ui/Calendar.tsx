import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  IconButton,
  Grid,
  useTheme,
  Tooltip,
} from '@mui/material';
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
} from '@mui/icons-material';
import { format, addMonths, subMonths, startOfMonth, endOfMonth, eachDayOfInterval, isSameMonth, isSameDay, isToday } from 'date-fns';
import dayjs, { Dayjs } from 'dayjs';
import { useSelector } from 'react-redux';
import { RootState } from '@/lib/redux/store';
import { Meeting } from '@/lib/redux/slices/meetingSlice';
import ScheduleCalendar from '@/components/ui/ScheduleCalendar';

interface CalendarProps {
  onMeetingSelect?: (meeting: Meeting) => void;
  selectedMeeting?: Meeting;
}

export default function Calendar({ onMeetingSelect, selectedMeeting }: CalendarProps) {
  const theme = useTheme();
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const meetings = useSelector((state: RootState) => state.meetings.meetings);

  const daysInMonth = eachDayOfInterval({
    start: startOfMonth(currentMonth),
    end: endOfMonth(currentMonth),
  });

  const previousMonth = () => {
    setCurrentMonth(subMonths(currentMonth, 1));
  };

  const nextMonth = () => {
    setCurrentMonth(addMonths(currentMonth, 1));
  };

  const getMeetingsForDate = (date: Date): Meeting[] => {
    return meetings.filter((meeting) => {
      const meetingDate = new Date(meeting.startTime);
      return isSameDay(meetingDate, date);
    });
  };

  return (
    <Paper elevation={3} sx={{ p: 2 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <IconButton onClick={previousMonth}>
          <ChevronLeftIcon />
        </IconButton>
        <Typography variant="h6">
          {format(currentMonth, 'MMMM yyyy')}
        </Typography>
        <IconButton onClick={nextMonth}>
          <ChevronRightIcon />
        </IconButton>
      </Box>

      <ScheduleCalendar></ScheduleCalendar>
    </Paper>
  );
}
