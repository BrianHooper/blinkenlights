import requests
from icalendar import Calendar, prop
from datetime import datetime
from BrianTools.Tools import ApiError, IsNullOrEmpty

class Event:
    def __init__(self, name, date):
        self.name = name
        self.date = date

    def toDict(self):
        return { "Name": self.name, "Date": self.date.strftime("%a - %m/%d") }

def ToEventDict(name, dtstart):
    if name is None or dtstart is None:
        return None

    if type(name) is not prop.vText:
        return None

    nameStr = str(name)
    if nameStr is None or len(nameStr) == 0:
        return None

    if type(dtstart) is not prop.vDDDTypes or dtstart.dt is None:
        return None

    if type(dtstart.dt) is datetime.date:
        if dtstart.dt < datetime.now().date():
            return None
        return Event(nameStr, dtstart.dt)
    elif type(dtstart.dt) is datetime:
        if dtstart.dt < datetime.now(dtstart.dt.tzinfo):
            return None
        return Event(nameStr, dtstart.dt.date())
    else:
        return None

def GetCalendar(userAccount, secret):
    if IsNullOrEmpty(userAccount) or IsNullOrEmpty(secret):
        return ApiError("API Headers are invalid")

    apiEndpoint = f"https://calendar.google.com/calendar/ical/{userAccount}/{secret}/basic.ics"

    response = requests.get(apiEndpoint)

    if response.status_code != 200:
        return ApiError("Failed to get response from Google/iCal API")

    if response.text is None or len(response.text) == 0:
        return ApiError("Google/iCal API response was null or empty")

    gcal = Calendar.from_ical(response.text)
    if gcal is None:
        return ApiError("Failed to create calendar from response text")
    
    events = []
    for c in gcal.walk("VEVENT"):
        event = ToEventDict(c.get('summary'), c.get('dtstart'))
        if event is not None:
            events.append(event)
    
    if len(events) == 0:
        return ApiError("Failed to parse any valid events")

    events_sorted = sorted(events, key=lambda x: x.date)
    events_filtered = list(map(lambda x: x.toDict(), events_sorted))

    if len(events_filtered) is None:
        return ApiError("Failed to parse any valid events")

    return { "Events": events_filtered }