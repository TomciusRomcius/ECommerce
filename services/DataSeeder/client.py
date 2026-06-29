import logging

import requests

import config

logger = logging.getLogger(__name__)


class ApiClient:
    def __init__(self) -> None:
        self._token: str | None = None
        self._session = requests.Session()

    def get_admin_token(self) -> str | None:
        try:
            response = self._session.post(
                config.KEYCLOAK_TOKEN_URL,
                data={
                    "client_id": config.KEYCLOAK_CLIENT_ID,
                    "client_secret": config.KEYCLOAK_CLIENT_SECRET,
                    "grant_type": "client_credentials",
                },
                timeout=config.REQUEST_TIMEOUT_SEC,
            )
            response.raise_for_status()
            token = response.json().get("access_token")
            if not token:
                logger.error("Keycloak token response missing access_token")
                return None
            self._token = token
            return token
        except requests.RequestException as exc:
            logger.error("Failed to obtain admin token: %s", exc)
            return None

    def _headers(self) -> dict[str, str]:
        if not self._token:
            raise RuntimeError("Admin token not set")
        return {"Authorization": f"Bearer {self._token}"}

    def get_json(self, path: str, params: dict | None = None) -> object | None:
        response = self._session.get(
            f"{config.API_BASE_URL}{path}",
            params=params,
            headers=self._headers(),
            timeout=config.REQUEST_TIMEOUT_SEC,
        )
        response.raise_for_status()
        return response.json()

    def post_json(self, path: str, body: dict) -> dict | None:
        try:
            response = self._session.post(
                f"{config.API_BASE_URL}{path}",
                json=body,
                headers=self._headers(),
                timeout=config.REQUEST_TIMEOUT_SEC,
            )
            if not response.ok:
                logger.warning(
                    "POST %s failed (%s): %s",
                    path,
                    response.status_code,
                    response.text,
                )
                return None
            if response.content:
                return response.json()
            return {}
        except requests.RequestException as exc:
            logger.warning("POST %s failed: %s", path, exc)
            return None
