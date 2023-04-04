def TrackingResult(provider=None, trackingNumber=None, name=None, status=None, eta=None):
    return {
        "Provider": provider,
        "TrackingNumber": trackingNumber,
        "Name": name,
        "Status": status,
        "ETA": eta
    }