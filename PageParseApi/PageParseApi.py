import uvicorn
from fastapi import FastAPI, Request, Body
import BrianTools
from BrianTools.Tools import Engine, ApiError
from GoogleCalendarApi import GetCalendar
from WikipediaApi import GetWikipedia
from YCombinatorApi import GetYCombinatorData
from RocketLaunchApi import GetRocketData
from AstronomyApi import GetAstroPicOfTheDay
from PackageTrackingApi import GetTrackingInfo
from IssTrackerApi import GetIssLocationImage
from WikipediaPotd import GetWikiPicOfTheDay

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
    return GetAstroPicOfTheDay()

@app.post("/packagetracking")
async def PackageTracking(requests: Request, Items = Body(...)):
    return GetTrackingInfo(requests.headers, Items)

@app.get("/isstracker")
def isstracker():
    return GetIssLocationImage()

@app.get("/wikipotd")
def wikipotd():
    return GetWikiPicOfTheDay()

if __name__ == "__main__":
    print(f"BrianTools version: {BrianTools.__version__}")
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5001, log_level="info")