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
import { fetchUsers } from '@/lib/redux/slices/userSlice';

export default function DashboardPage() {
  const dispatch = useDispatch<AppDispatch>();
  const users = useSelector((state: RootState) => state.users.users);

  useEffect(() => {
    dispatch(fetchUsers());
  }, [dispatch]);

  return (
    <DashboardLayout>


      <TableWrapper>
        <ScrollableTableContainer component={Paper}>
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                <TableCell>User</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Role</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.id}>
                  <TableCell>
                    <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                      <Typography variant="body2" sx={{ fontWeight: 500 }}>
                        {`${user.firstName} ${user.lastName}`}
                      </Typography>
                      <Typography variant="caption" color="textSecondary">
                        {user.username}
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell>{user.email || '-'}</TableCell>
                  <TableCell>
                    <Chip
                      label={user.status}
                      size="small"
                      sx={{
                        bgcolor: user.status === 'active' ? '#E8F5E9' : '#FFEBEE',
                        color: user.status === 'active' ? '#2E7D32' : '#C62828',
                        borderRadius: '4px',
                        fontWeight: 500,
                        height: '24px',
                        '.MuiChip-label': {
                          padding: '0 8px'
                        }
                      }}
                    />
                  </TableCell>
                  <TableCell>{user.role}</TableCell>
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
