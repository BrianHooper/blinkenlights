import requests
import os
import json
from BrianTools.Tools import IsNullOrEmpty
from datetime import datetime, timedelta
from oauthlib.oauth2 import BackendApplicationClient
from requests_oauthlib import OAuth2Session
from TrackingResult import TrackingResult

upsAuthenticationCacheFilename = "UpsAuthenticationCache.json"

def GetCachedUpsAuthentication():
        if not os.path.exists(upsAuthenticationCacheFilename):
            return None
        
        with open(upsAuthenticationCacheFilename, "r", encoding="utf-8") as infile:
            token_response = json.load(infile)
        if token_response is None:
            return None
        
        if "issued_at" not in token_response or IsNullOrEmpty(token_response["issued_at"]):
            return None
        if "expires_in" not in token_response or IsNullOrEmpty(token_response["expires_in"]):
            return None

        issued_at_epoch = int(token_response["issued_at"]) / 1000
        issued_at = datetime.fromtimestamp(issued_at_epoch)
        expires_in = int(token_response["expires_in"])
        expires_at = issued_at + timedelta(seconds=expires_in)
        now = datetime.now()
        if expires_at <= now:
            return None
        
        if "access_token" not in token_response or IsNullOrEmpty(token_response["access_token"]):
            return None
        return token_response["access_token"]

def GetUpsAuthentication(UPS_OATH_CLIENT_ID, UPS_OATH_CLIENT_SECRET):
    cachedToken = GetCachedUpsAuthentication()
    if not IsNullOrEmpty(cachedToken):
        return cachedToken
    
    client = BackendApplicationClient(client_id=UPS_OATH_CLIENT_ID)
    oauth = OAuth2Session(client=client)
    token_response = oauth.fetch_token(token_url="https://onlinetools.ups.com/security/v1/oauth/token", 
                                        client_id=UPS_OATH_CLIENT_ID,
                                        client_secret=UPS_OATH_CLIENT_SECRET)
    if token_response is None or "access_token" not in token_response or IsNullOrEmpty(token_response["access_token"]):
        return None
    
    with open(upsAuthenticationCacheFilename, "w", encoding="utf-8") as outfile:
        json.dump(token_response, outfile)
    return token_response["access_token"]

def GetUpsTracking(trackingNumber, name, UPS_OATH_CLIENT_ID, UPS_OATH_CLIENT_SECRET):
    accessToken = GetUpsAuthentication(UPS_OATH_CLIENT_ID, UPS_OATH_CLIENT_SECRET)
    if IsNullOrEmpty(accessToken):
        return TrackingResult("UPS", trackingNumber, name, "Failed to get auth token", None)

    t0 = datetime(1, 1, 1)
    now = datetime.utcnow()
    seconds = (now - t0).total_seconds()
    ticks = int(seconds * 10**7)
    url = "https://onlinetools.ups.com/api/track/v1/details/{0}".format(trackingNumber)

    headers = {
        "Content-Type": "application/json; charset=utf-8",
        "transId": str(ticks),
        "transactionSrc": "testing",
        "Authorization": "Bearer {0}".format(accessToken),
    }

    response = requests.get(url, headers=headers)
    if response is None or response.status_code != 200:
        return TrackingResult("UPS", trackingNumber, name, "API failed", None)

    return TrackingResult("UPS", trackingNumber, name, response.text, None)