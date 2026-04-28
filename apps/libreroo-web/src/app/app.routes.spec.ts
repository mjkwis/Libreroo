import { routes } from './app.routes';

describe('app routes', () => {
  it('should expose phase 1 public routes', () => {
    const paths = routes.map((route) => route.path);
    expect(paths).toContain('catalog');
    expect(paths).toContain('member');
    expect(paths).toContain('loans');
  });
});
