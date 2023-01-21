from Constants import *

from bs4 import BeautifulSoup
import time
from selenium import webdriver
from selenium.webdriver.chrome.options import Options


class Engine:
	def  __init__(self):
		self.CHROMEDRIVER_PATH = 'chromedriver'
		user_agent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36'
		chrome_options = Options()
		chrome_options.add_argument("--headless")
		chrome_options.add_argument("--no-sandbox")
		chrome_options.add_argument("disable-gpu")
		chrome_options.add_argument("window-size=1400,2100")
		chrome_options.add_argument(f'user-agent={user_agent}')
		chrome_options.add_experimental_option('excludeSwitches', ['enable-logging'])
		self.chrome_options = chrome_options

	def GetSoup(self, url):
		driver = webdriver.Chrome(executable_path=self.CHROMEDRIVER_PATH, options=self.chrome_options)
		driver.get(url)
		time.sleep(3)
		page_html = driver.page_source
		soup = BeautifulSoup(page_html, 'html.parser')
		driver.close()
		return soup