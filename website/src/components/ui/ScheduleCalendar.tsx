'use client'

import { useNextCalendarApp, ScheduleXCalendar } from '@schedule-x/react'
import {
  CalendarConfig,
  createViewDay,
  createViewMonthAgenda,
  createViewMonthGrid,
  createViewWeek,
} from '@schedule-x/calendar'
import { createEventsServicePlugin } from '@schedule-x/events-service'

import '@schedule-x/theme-default/dist/index.css'
import { useCallback, useEffect } from "react";
import { Meeting } from '@/lib/redux/slices/meetingSlice'
import dayjs from 'dayjs'

interface ScheduleCalendarProps {
  onMeetingSelect?: (meetingId: string) => void;
  selectedMeeting?: Meeting;
  meetings?: Meeting[];
}

function CalendarApp({ onMeetingSelect, selectedMeeting, meetings }: ScheduleCalendarProps) {
  const eventsServicePlugin = createEventsServicePlugin();
  const plugins = [eventsServicePlugin];

  const calendar = useNextCalendarApp({
    views: [createViewDay(), createViewWeek(), createViewMonthGrid(), createViewMonthAgenda()],
    callbacks: {
      /**
       * Is called when an event is clicked
       * */
      onEventClick(calendarEvent) {
        onMeetingSelect && onMeetingSelect(calendarEvent.id.toString());
      },
    },
    selectedDate: selectedMeeting ? dayjs(selectedMeeting.startTime).format('YYYY-MM-DD') : dayjs().format('YYYY-MM-DD'),
    events: meetings?.map(meeting => {
      return {
        id: meeting.id,
        title: meeting.title,
        start: dayjs(meeting.startTime).format('YYYY-MM-DD HH:mm'),
        end: dayjs(meeting.endTime).format('YYYY-MM-DD HH:mm'),
      }
    }) ?? [],
  }, plugins);

  useEffect(() => {
    calendar?.events.set(meetings?.map(meeting => {
      return {
        id: meeting.id,
        title: meeting.title,
        start: dayjs(meeting.startTime).format('YYYY-MM-DD HH:mm'),
        end: dayjs(meeting.endTime).format('YYYY-MM-DD HH:mm'),
      }
    }) ?? []);
    // get all events
    // calendar?.events.getAll();
  }, [meetings])

  return (
    <div>
      <ScheduleXCalendar calendarApp={calendar} />
    </div>
  )
}

export default CalendarApp