#!/bin/bash
set -e

host="$1"
shift
cmd="$@"

until PGPASSWORD=$POSTGRES_PASSWORD psql -h "$host" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c '\q'; do
  echo "Esperando o Postgres em $host..."
  sleep 2
done

echo "Postgres pronto! Rodando migrations..."
exec $cmd