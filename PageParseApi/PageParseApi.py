
import uvicorn
from fastapi import FastAPI, Request
from BrianTools import Engine, ApiError
from GoogleCalendarApi import GetCalendar
from WikipediaApi import GetWikipedia

app = FastAPI()

@app.get("/wikipedia")
def wikipedia():
    return GetWikipedia()

@app.get("/googlecalendar")
async def GoogleCalendar(request: Request):
    userAccount = request.headers.get("X-user-account")
    if userAccount is None or len(userAccount) == 0:
        return ApiError("Missing header: X-user-account")
    secret = request.headers.get("X-user-secret")
    if secret is None or len(secret) == 0:
        return ApiError("Missing header: X-user-secret")
    return GetCalendar(userAccount, secret)
    
if __name__ == "__main__":
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5001, log_level="info")