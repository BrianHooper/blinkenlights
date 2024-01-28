import requests
import os
import json
from BrianTools.Tools import IsNullOrEmpty
from datetime import datetime, timedelta
from oauthlib.oauth2 import BackendApplicationClient
from requests_oauthlib import OAuth2Session
from TrackingResult import TrackingResult

AuthenticationCacheFilename = "FedexAuthenticationCache.json"

def GetCachedAuthentication():
        if not os.path.exists(AuthenticationCacheFilename):
            return None
        
        with open(AuthenticationCacheFilename, "r", encoding="utf-8") as infile:
            token_response = json.load(infile)
        if token_response is None:
            return None
        
        if "issued_at" not in token_response or "expires_in" not in token_response:
            return None

        issued_at_epoch = token_response["issued_at"]
        issued_at = datetime.fromtimestamp(issued_at_epoch)
        expires_in = int(token_response["expires_in"])
        expires_at = issued_at + timedelta(seconds=expires_in)
        now = datetime.now()
        if expires_at <= now:
            return None
        
        if "access_token" not in token_response or IsNullOrEmpty(token_response["access_token"]):
            return None
        return token_response["access_token"]

def GetAuthentication(OATH_CLIENT_ID, OATH_CLIENT_SECRET):
    cachedToken = GetCachedAuthentication()
    if not IsNullOrEmpty(cachedToken):
        return cachedToken
    
    url = "https://apis-sandbox.fedex.com/oauth/token"
    payload = f"grant_type=client_credentials&client_id={OATH_CLIENT_ID}&client_secret={OATH_CLIENT_SECRET}"
    headers = {"Content-Type": "application/x-www-form-urlencoded"}

    response = requests.request("POST", url, data=payload, headers=headers)
    if response is None or response.status_code != 200 or IsNullOrEmpty(response.text):
        return None
    token_response = json.loads(response.text)
    if token_response is None or "access_token" not in token_response or IsNullOrEmpty(token_response["access_token"]):
        return None
    token_response["issued_at"] = int(datetime.now().timestamp())

    with open(AuthenticationCacheFilename, "w", encoding="utf-8") as outfile:
        json.dump(token_response, outfile)
    return token_response["access_token"]

def GetFedexTracking(trackingNumber, name, OATH_CLIENT_ID, OATH_CLIENT_SECRET):
    accessToken = GetAuthentication(OATH_CLIENT_ID, OATH_CLIENT_SECRET)
    if IsNullOrEmpty(accessToken):
        return TrackingResult("FEDEX", trackingNumber, name, "Failed to get auth token", None)

    url = "https://apis-sandbox.fedex.com/track/v1/trackingnumbers"

    payload = {"trackingInfo": [{"trackingNumberInfo": {"trackingNumber": trackingNumber}}]}
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {accessToken}"
    }

    response = requests.request("POST", url, json=payload, headers=headers)
    if response is None or response.status_code != 200:
        return TrackingResult("FEDEX", trackingNumber, name, "API failed", None)

    return TrackingResult("FEDEX", trackingNumber, name, response.text, None)