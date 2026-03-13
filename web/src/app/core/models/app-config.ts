export interface ApiConfig {
  apiBaseUrl: string;
  whatsappNumber: string;
  instagramUrl: string;
  gaMeasurementId: string;
  searchConsoleVerification: string;
}

// API URL will be replaced at build time by Railway
export const apiConfig: ApiConfig = {
  apiBaseUrl: '__API_BASE_URL__',
  whatsappNumber: '971500000000',
  instagramUrl: 'https://instagram.com/khanhomefloral',
  gaMeasurementId: 'G-XXXXXXXXXX',
  searchConsoleVerification: 'google-site-verification-placeholder'
};

