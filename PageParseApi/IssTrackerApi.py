import requests
from BrianTools.Tools import ApiError, IsNullOrEmpty
import json
import math
import requests
import json
import matplotlib
import matplotlib.pyplot as plt
import cartopy.crs as ccrs
import cartopy.feature as cfeature
from pathlib import Path
from datetime import datetime

API_URL = "http://api.open-notify.org/iss-now.json"
API_HEADERS = {
    "Content-Type": "application/json"
}
FILENAME = "iss_location.png"

def CosDegrees(angle):
    return math.cos(angle * (math.pi / 180))

def SinDegrees(angle):
    return math.sin(angle * (math.pi / 180))

class Coordinates:
    def __init__(self, latitude: float, longitude: float) -> None:
        self.latitude = latitude
        self.longitude = longitude

        calc_radius = CosDegrees(latitude)
        self.x = calc_radius * SinDegrees(longitude)
        self.y = SinDegrees(latitude)
        self.z = calc_radius * CosDegrees(longitude)

def GetIssCoordinatesApi():
    response = requests.request("GET", API_URL, headers=API_HEADERS)
    if response is None or response.status_code != 200 or len(response.text) == 0:
        return None

    try:
        data = json.loads(response.text)
        latitude = float(data["iss_position"]["latitude"])
        longitude = float(data["iss_position"]["longitude"])
    except Exception:
        return None
    return Coordinates(latitude, longitude)

def PlotPoint(coordinates: Coordinates, label: str, ax: object, color: str) -> None:
    textlat = coordinates.latitude + 5
    if textlat >= 90:
        textlat = coordinates.latitude - 7

    ax.scatter(coordinates.longitude, coordinates.latitude, color=color, edgecolors="black", marker="o", transform=ccrs.Geodetic())
    plt.text(coordinates.longitude, textlat, label,
         horizontalalignment="center", transform=ccrs.Geodetic(), 
         bbox=dict(facecolor="white", edgecolor="black", boxstyle="round,pad=0.2"))

def GetIssLocationImage(headless=True):
    iss_coordinates = GetIssCoordinatesApi()
    if iss_coordinates is None:
        return ApiError("iss-now api failed")
    # home_coordinates = Coordinates(0, 0)
    
    if headless:
        matplotlib.use("agg")
    fig = plt.figure(frameon = False)
    ax = fig.add_subplot(1, 1, 1, projection=ccrs.Orthographic(iss_coordinates.longitude, iss_coordinates.latitude))

    ax.add_feature(cfeature.OCEAN, zorder=0)
    ax.add_feature(cfeature.LAND, zorder=0, edgecolor="black")
    ax.set_global()

    PlotPoint(iss_coordinates, "ISS", ax, "orange")
    # plot_point(home_coordinates, "HOME", ax, "orange")

    image_path = str(Path(__file__).parent / FILENAME)
    fig.savefig(image_path, bbox_inches="tight", transparent=True)
    
    if not headless:
        plt.show()
    update_datetime = datetime.now().strftime("%m/%d/%Y %I:%M%p")
    return { 
        "image_path": image_path,
        "last_update_time": update_datetime,
        "latitude": iss_coordinates.latitude,
        "longitude": iss_coordinates.longitude
    }

if __name__ == "__main__":
    data = GetIssLocationImage(headless=True)
    pass