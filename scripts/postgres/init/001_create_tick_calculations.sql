CREATE TABLE IF NOT EXISTS tick_calculations (
    id BIGSERIAL PRIMARY KEY,
    symbol TEXT NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL,
    open NUMERIC NOT NULL,
    high NUMERIC NOT NULL,
    low NUMERIC NOT NULL,
    close NUMERIC NOT NULL,
    daily_moving_average NUMERIC NOT NULL,
    volume BIGINT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_tick_calc_symbol_time
    ON tick_calculations(symbol, timestamp);
