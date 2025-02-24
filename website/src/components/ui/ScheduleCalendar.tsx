'use client'

import { useNextCalendarApp, ScheduleXCalendar } from '@schedule-x/react'
import {
  createViewDay,
  createViewMonthAgenda,
  createViewMonthGrid,
  createViewWeek,
} from '@schedule-x/calendar'
import { createEventsServicePlugin } from '@schedule-x/events-service'

import '@schedule-x/theme-default/dist/index.css'
import { useEffect } from "react";
import { Meeting } from '@/lib/redux/slices/meetingSlice'
import dayjs from 'dayjs'

interface ScheduleCalendarProps {
  onMeetingSelect?: (meetingId: string) => void;
  selectedMeeting?: Meeting;
  mettings?: Meeting[];
}

function CalendarApp({ onMeetingSelect, selectedMeeting, mettings }: ScheduleCalendarProps) {
  const plugins = [createEventsServicePlugin()]

  const calendar = useNextCalendarApp({
    views: [createViewDay(), createViewWeek(), createViewMonthGrid(), createViewMonthAgenda()],
    callbacks: {
      /**
       * Is called when an event is clicked
       * */
      onEventClick(calendarEvent) {
        console.log('onEventClick', calendarEvent);
        onMeetingSelect && onMeetingSelect(calendarEvent.id.toString());
      },
    },
    selectedDate: selectedMeeting ? dayjs(selectedMeeting.startTime).format('YYYY-MM-DD') : dayjs().format('YYYY-MM-DD'),
    events: mettings?.map(meeting => {
      return {
        id: meeting.id,
        title: meeting.title,
        start: dayjs(meeting.startTime).format('YYYY-MM-DD HH:mm'),
        end: dayjs(meeting.endTime).format('YYYY-MM-DD HH:mm'),
      }
    }) ?? [
        {
          id: '1',
          title: 'Meeting 1',
          start: '2025-02-24',
          end: '2025-02-25',
        },
      ],
  }, plugins)

  useEffect(() => {
    // get all events
    calendar?.events.getAll()
  }, [])

  return (
    <div>
      <ScheduleXCalendar calendarApp={calendar} />
    </div>
  )
}

export default CalendarApp