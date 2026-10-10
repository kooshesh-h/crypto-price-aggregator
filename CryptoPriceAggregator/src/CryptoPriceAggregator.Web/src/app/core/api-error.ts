import { HttpErrorResponse } from '@angular/common/http';

export function apiErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Something went wrong. Please try again.';
  }
  if (error.status === 0) {
    return 'Can not reach the API Chek that the API is running and try again.';
  }
  const serverMessage = error.error?.error;
  if (typeof serverMessage === 'string' && serverMessage.trim()) {
    return serverMessage;
  }
  if (error.status === 502) {
    return 'A price provider is unavailable Please try again shortly.';
  }
  return `Request failed (${error.status}). Please try again.`;
}
