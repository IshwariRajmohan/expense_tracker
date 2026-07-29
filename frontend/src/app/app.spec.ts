import { describe, expect, it } from 'vitest';
import { routes } from './app.routes';

describe('App routes', () => {
  it('should expose the login and user routes', () => {
    const paths = routes.map((route) => route.path);

    expect(paths).toContain('');
    expect(paths).toContain('user');
  });
});
