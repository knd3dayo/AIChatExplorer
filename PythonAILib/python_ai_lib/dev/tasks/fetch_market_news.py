# filename: fetch_market_news.py
import requests
from bs4 import BeautifulSoup

def fetch_news(ticker):
    url = f"https://finance.yahoo.com/quote/{ticker}/news?p={ticker}"
    response = requests.get(url)
    soup = BeautifulSoup(response.text, 'html.parser')
    headlines = soup.find_all('h3', class_='Mb(5px)')
    
    news_items = []
    for headline in headlines[:5]:  # Get top 5 news items
        news_items.append(headline.get_text())
        
    return news_items

# Fetching news for Nvidia and Tesla
nvidia_news = fetch_news('NVDA')
tesla_news = fetch_news('TSLA')

print("Nvidia News:")
for item in nvidia_news:
    print("-", item)

print("\nTesla News:")
for item in tesla_news:
    print("-", item)