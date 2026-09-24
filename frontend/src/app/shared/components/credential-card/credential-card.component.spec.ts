import { CredentialCardComponent } from './credential-card.component';
import { CredentialDetail } from '../../../core/models/credential.model';
import { MemberCategory, CredentialStatus } from '../../../core/models/enums';

// renovacion-credencial-activa: "Vencida" es un estado visual derivado de
// validUntil, no de credentialStatus (que no se toca al renovar).
describe('CredentialCardComponent#isExpired', () => {
  const base: CredentialDetail = {
    id: 'id-1',
    photo: 'https://cdn.futbol.com.ar/socios/test.jpg',
    firstName: 'Juan',
    lastName: 'Pérez',
    category: MemberCategory.Adulto,
    memberNumber: '000001',
    validFrom: new Date().toISOString(),
    validUntil: new Date().toISOString(),
    status: CredentialStatus.Active,
    issuer: 'did:test:issuer',
    proofType: 'HMAC-SHA256'
  };

  function build(validUntil: string): CredentialCardComponent {
    const component = new CredentialCardComponent();
    component.credential = { ...base, validUntil };
    return component;
  }

  it('no está vencida cuando validUntil es futuro, aunque status sea active', () => {
    const future = new Date(Date.now() + 1000 * 60 * 60 * 24 * 30).toISOString();
    expect(build(future).isExpired).toBeFalse();
  });

  it('está vencida cuando validUntil ya pasó, aunque status siga siendo active', () => {
    const past = new Date(Date.now() - 1000 * 60 * 60 * 24).toISOString();
    expect(build(past).isExpired).toBeTrue();
  });
});
