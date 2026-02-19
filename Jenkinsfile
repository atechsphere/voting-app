pipeline {
    agent any
    
    environment {
        REGISTRY = 'localhost:5000'
        APP_IMAGE = "${REGISTRY}/voting-app"
        MYSQL_IMAGE = "${REGISTRY}/mysql"
        IMAGE_TAG = "${BUILD_NUMBER}"
    }
    
    stages {
        stage('Cleanup Existing Infrastructure') {
            steps {
                echo '🧹 Cleaning up existing infrastructure...'
                script {
                    sh '''
                        echo "=== Stopping all containers ==="
                        docker stop voting-app voting-mysql registry 2>/dev/null || true
                        
                        echo "=== Removing all containers ==="
                        docker rm voting-app voting-mysql registry 2>/dev/null || true
                        
                        echo "=== Removing application images ==="
                        docker rmi localhost:5000/voting-app:latest 2>/dev/null || true
                        docker rmi localhost:5000/mysql:8.0 2>/dev/null || true
                        docker rmi localhost:5000/mysql:latest 2>/dev/null || true
                        
                        echo "=== Removing volumes ==="
                        docker volume rm voting_db_data_v2 2>/dev/null || true
                        docker volume rm registry_data 2>/dev/null || true
                        
                        echo "=== Removing networks ==="
                        docker network rm voting-net-v2 2>/dev/null || true
                        
                        echo "=== Cleanup complete ==="
                    '''
                }
            }
        }
        
        stage('Setup Local Registry') {
            steps {
                echo '📦 Setting up local Docker registry...'
                script {
                    sh '''
                        docker volume create registry_data || true
                        docker run -d -p 5000:5000 --name registry --restart unless-stopped \
                            -v registry_data:/var/lib/registry registry:2
                        sleep 5
                    '''
                }
            }
        }
        
        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                dir('VotingApp') {
                    sh 'dotnet restore'
                }
            }
        }
        
        stage('Build Application') {
            steps {
                dir('VotingApp') {
                    sh 'dotnet build --configuration Release --no-restore'
                }
            }
        }
        
        stage('Build Docker Images') {
            steps {
                script {
                    dir('VotingApp') {
                        sh "docker build -t ${APP_IMAGE}:${IMAGE_TAG} -t ${APP_IMAGE}:latest ."
                    }
                    sh """
                        docker pull mysql:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:latest
                    """
                }
            }
        }
        
        stage('Push to Local Registry') {
            steps {
                sh """
                    docker push ${APP_IMAGE}:${IMAGE_TAG} || true
                    docker push ${APP_IMAGE}:latest
                    docker push ${MYSQL_IMAGE}:8.0
                    docker push ${MYSQL_IMAGE}:latest
                """
            }
        }
        
        stage('Deploy with Docker Compose') {
            steps {
                script {
                    sh '''
                        docker network create voting-net-v2 2>/dev/null || true
                        docker compose pull || true
                        docker compose up -d
                        sleep 20
                    '''
                }
            }
        }
        
        stage('Wait for MySQL') {
            steps {
                script {
                    sh '''
                        for i in {1..30}; do
                            if docker exec voting-mysql mysqladmin ping -h localhost -u root -pRootPass123! 2>/dev/null; then
                                echo "✓ MySQL is ready"
                                exit 0
                            fi
                            sleep 2
                        done
                    '''
                }
            }
        }
        
        stage('Wait for Application') {
            steps {
                script {
                    sh '''
                        for i in {1..30}; do
                            HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8087/ 2>/dev/null || echo "000")
                            if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "302" ]; then
                                echo "✓ Application is ready"
                                exit 0
                            fi
                            sleep 3
                        done
                        docker compose logs --tail=30 voting-app
                        exit 1
                    '''
                }
            }
        }
        
        stage('Verify Deployment') {
            steps {
                sh '''
                    echo "=== Container Status ==="
                    docker compose ps
                    echo "=== Registry Contents ==="
                    curl -s http://localhost:5000/v2/_catalog
                '''
            }
        }
    }
    
    post {
        success {
            echo '✅ DEPLOYMENT SUCCESSFUL!'
            echo '🌐 Application: http://localhost:8087'
        }
        failure {
            echo '❌ DEPLOYMENT FAILED!'
            sh 'docker-compose logs --tail=50 || true'
        }
        always {
            sh 'docker image prune -f || true'
        }
    }
}
