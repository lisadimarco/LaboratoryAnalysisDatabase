import requests

url = "http://localhost:5000/api/labtest"
data = {
    "test_id": 1,
    "category_id": 2,
    "test_name": "Test Name",
    "abbreviation": "TN",
    "unit": "mg/dl",
    "reference_min": 5.0,
    "reference_max": 10.0,
    "gender_specific": False,
    "age_specific": False,
    "fasting_required": True
}
response = requests.post(url, json=data)
if response.status_code == 200:
    print("Data inserted successfully.")
