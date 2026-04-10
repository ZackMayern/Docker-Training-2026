{{- define "weather-app.name" -}}
weather-app
{{- end -}}

{{- define "weather-app.namespace" -}}
{{- .Values.namespace.name -}}
{{- end -}}

{{- define "weather-app.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" -}}
{{- end -}}

{{- define "weather-app.labels" -}}
app.kubernetes.io/name: {{ include "weather-app.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
helm.sh/chart: {{ include "weather-app.chart" . }}
{{- end -}}

{{- define "weather-app.selectorLabels" -}}
app.kubernetes.io/name: {{ include "weather-app.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}