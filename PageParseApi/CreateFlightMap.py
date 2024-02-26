import matplotlib.pyplot as plt
import cartopy.crs as ccrs
import cartopy.feature as cfeature


fig = plt.figure()
ax = fig.add_subplot(1, 1, 1, projection=ccrs.PlateCarree())

ax.set_extent([-122.22,-122.41,47.37,47.49], crs=ccrs.PlateCarree())
ax.stock_img()
ax.add_feature(cfeature.LAND)
ax.add_feature(cfeature.OCEAN)
ax.add_feature(cfeature.COASTLINE)
ax.add_feature(cfeature.BORDERS, linestyle='dotted')
ax.add_feature(cfeature.LAKES, alpha=0.5)
ax.add_feature(cfeature.RIVERS)
plt.show()