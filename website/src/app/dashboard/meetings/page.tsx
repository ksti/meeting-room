'use client';

import { useEffect, useState } from 'react';
import {
  Grid,
  Paper,
  Typography,
  Box,
  TableContainer,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Chip,
} from '@mui/material';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/lib/redux/store';
import DashboardLayout from '@/components/layout/DashboardLayout';
import { styled } from 'styled-components';
import { fetchMeetings } from '@/lib/redux/slices/meetingSlice';

export default function DashboardPage() {
  const dispatch = useDispatch<AppDispatch>();
  const meetings = useSelector((state: RootState) => state.meetings.meetings);

  useEffect(() => {
    dispatch(fetchMeetings());
  }, [dispatch]);

  return (
    <DashboardLayout>


      <TableWrapper>
        <ScrollableTableContainer component={Paper}>
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                <TableCell>Title</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Organizer</TableCell>
                <TableCell>Attendees</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {meetings.map((meeting) => (
                <TableRow key={meeting.id}>
                  <TableCell>
                    <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                      <Typography variant="caption" color="textSecondary">
                        {meeting.title}
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell>{meeting.description}</TableCell>
                  <TableCell>{meeting.organizer}</TableCell>
                  <TableCell>{meeting.attendees.join(',')}</TableCell>
                  <TableCell>{meeting.status}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </ScrollableTableContainer>
      </TableWrapper>


    </DashboardLayout>
  );
}

const TableWrapper = styled(Box)({
  position: 'relative',
  display: 'flex',
  flexDirection: 'column',
  overflow: 'hidden',
  height: 'auto',
  minHeight: '200px',
});

const ScrollableTableContainer = styled(TableContainer)({
  overflow: 'auto',
  maxHeight: 'calc(100vh - 300px)',
  '& .MuiTable-root': {
    minWidth: 'auto',
    width: '100%',
    tableLayout: 'fixed'
  },
  '& .MuiTableCell-root': {
    padding: '12px 16px',
    '&:first-of-type': {
      paddingLeft: '24px'
    },
    '&:last-of-type': {
      paddingRight: '24px'
    }
  }
}) as typeof TableContainer;
