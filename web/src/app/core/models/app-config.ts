export interface ApiConfig {
  apiBaseUrl: string;
  whatsappNumber: string;
  instagramUrl: string;
  gaMeasurementId: string;
  searchConsoleVerification: string;
}

export const apiConfig: ApiConfig = {
  apiBaseUrl: 'http://localhost:5001/api',
  whatsappNumber: '971500000000',
  instagramUrl: 'https://instagram.com/khanhomefloral',
  gaMeasurementId: 'G-XXXXXXXXXX',
  searchConsoleVerification: 'google-site-verification-placeholder'
};

