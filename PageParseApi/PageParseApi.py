import uvicorn
from fastapi import FastAPI, Request, Body
from BrianTools import Engine, ApiError
from GoogleCalendarApi import GetCalendar
from WikipediaApi import GetWikipedia
from YCombinatorApi import GetYCombinatorData
from RocketLaunchApi import GetRocketData
from AstronomyApi import GetPicOfTheDay
from PackageTrackingApi import GetTrackingInfo

app = FastAPI()

@app.get("/wikipedia")
def wikipedia():
    try:
        wikiData = GetWikipedia()
        return wikiData
    except Exception as e:
        return ApiError(f"Exception: {e}")
    
@app.get("/rockets")
def rockets():
    try:
        rocketData = GetRocketData()
        return rocketData
    except Exception as e:
        return ApiError(f"Exception: {e}")
    
@app.get("/ycombinator")
def ycombinator():
    try:
        ycombinatorData = GetYCombinatorData()
        return ycombinatorData
    except Exception as e:
        return ApiError(f"Exception: {e}")

@app.get("/googlecalendar")
async def GoogleCalendar(request: Request):
    userAccount = request.headers.get("X-user-account")
    if userAccount is None or len(userAccount) == 0:
        return ApiError("Missing header: X-user-account")
    secret = request.headers.get("X-user-secret")
    if secret is None or len(secret) == 0:
        return ApiError("Missing header: X-user-secret")
    return GetCalendar(userAccount, secret)

@app.get("/astronomypotd")
def astronomy():
    return GetPicOfTheDay()

@app.post("/packagetracking")
async def PackageTracking(requests: Request, Items = Body(...)):
    return GetTrackingInfo(requests.headers, Items)

if __name__ == "__main__":
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5001, log_level="info")