pipeline {
    agent any
    
    environment {
        REGISTRY = 'localhost:5000'
        APP_IMAGE = "${REGISTRY}/voting-app"
        MYSQL_IMAGE = "${REGISTRY}/mysql"
        IMAGE_TAG = "${BUILD_NUMBER}"
    }
    
    stages {
        stage('Cleanup Infrastructure') {
            steps {
                echo '🧹 Wiping old environment for a clean start...'
                script {
                    sh '''
                        docker stop voting-app voting-mysql registry 2>/dev/null || true
                        docker rm voting-app voting-mysql registry 2>/dev/null || true
                        docker volume rm voting_db_data_v2 2>/dev/null || true
                        docker network rm voting-net-v2 2>/dev/null || true
                    '''
                }
            }
        }
        
        stage('Setup Local Registry') {
            steps {
                sh 'docker run -d -p 5000:5000 --name registry --restart unless-stopped registry:2 2>/dev/null || true'
                sleep 5
            }
        }
        
        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore & Build App') {
            steps {
                dir('VotingApp') {
                    sh 'dotnet restore'
                    sh 'dotnet build --configuration Release --no-restore'
                }
            }
        }
        
        stage('Build & Push Docker Images') {
            steps {
                script {
                    dir('VotingApp') {
                        sh "docker build -t ${APP_IMAGE}:latest -t ${APP_IMAGE}:${IMAGE_TAG} ."
                    }
                    sh """
                        docker pull mysql:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:8.0
                        docker push ${APP_IMAGE}:latest
                        docker push ${MYSQL_IMAGE}:8.0
                    """
                }
            }
        }
        
        stage('Deploy Application') {
            steps {
                script {
                    sh '''
                        docker compose pull
                        docker compose up -d
                        echo "⏳ Waiting 45 seconds for MySQL Init and EF Migrations..."
                        sleep 45
                    '''
                }
            }
        }
        
        stage('Wait for MySQL Health') {
            steps {
                script {
                    sh '''
                        for i in {1..20}; do
                            if docker exec voting-mysql mysqladmin ping -h localhost -u root -pRootPass123! 2>/dev/null; then
                                echo "✅ MySQL is responding!"
                                exit 0
                            fi
                            echo "Waiting for MySQL socket..."
                            sleep 3
                        done
                        exit 1
                    '''
                }
            }
        }
        
        stage('Wait for Voting App') {
            steps {
                script {
                    sh '''
                        for i in {1..20}; do
                            HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8087/ || echo "000")
                            if [ "$HTTP_CODE" = "200" ]; then
                                echo "✅ App is UP (HTTP 200)!"
                                exit 0
                            fi
                            echo "Waiting for App (Current HTTP: $HTTP_CODE)..."
                            sleep 5
                        done
                        echo "❌ App failed to start. Logs:"
                        docker compose logs voting-app
                        exit 1
                    '''
                }
            }
        }
    }
    
    post {
        success {
            echo '🚀 DEPLOYMENT SUCCESSFUL! Access at http://localhost:8087'
        }
        failure {
            echo '❌ DEPLOYMENT FAILED! Dumping logs...'
            sh 'docker compose logs --tail=100'
        }
        always {
            sh 'docker image prune -f || true'
        }
    }
}
