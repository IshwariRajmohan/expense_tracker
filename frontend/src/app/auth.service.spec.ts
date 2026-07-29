import { describe, expect, it } from 'vitest';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  it('accepts the expected credentials', () => {
    const service = new AuthService();

    expect(service.validateCredentials('himesh', '123')).toBe(true);
    expect(service.validateCredentials('himesh', 'wrong')).toBe(false);
  });
});
