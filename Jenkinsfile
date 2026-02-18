pipeline {
    agent any
    
    environment {
        REGISTRY = 'localhost:5000'
        APP_IMAGE = "${REGISTRY}/voting-app"
        MYSQL_IMAGE = "${REGISTRY}/mysql"
        IMAGE_TAG = "${BUILD_NUMBER}"
        DOCKER_NETWORK = 'voting-net-v2'
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out source code...'
                checkout scm
            }
        }
        
        stage('Setup Local Registry') {
            steps {
                script {
                    echo 'Setting up local Docker registry...'
                    sh '''
                        if ! docker ps | grep -q registry; then
                            docker run -d -p 5000:5000 --name registry \\
                                -v registry_data:/var/lib/registry \\
                                registry:2 || true
                        fi
                        sleep 3
                    '''
                }
            }
        }
        
        stage('Build .NET Application') {
            steps {
                echo 'Building .NET application...'
                dir('VotingApp') {
                    sh '''
                        dotnet restore
                        dotnet build --configuration Release --no-restore
                    '''
                }
            }
        }
        
        stage('Build Docker Images') {
            steps {
                echo 'Building Docker images...'
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
        
        stage('Security Scan') {
            steps {
                echo 'Running security scan...'
                script {
                    sh '''
                        # Skip trivy installation if it fails - not critical
                        if which trivy > /dev/null 2>&1; then
                            trivy image --severity HIGH,CRITICAL ${APP_IMAGE}:${IMAGE_TAG} || true
                        else
                            echo "Skipping security scan - trivy not installed"
                        fi
                    '''
                }
            }
        }
        
        stage('Push to Local Registry') {
            steps {
                echo 'Pushing images to local registry...'
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
                        docker stop voting-mysql voting-app 2>/dev/null || true
                        docker rm voting-mysql voting-app 2>/dev/null || true
                        docker-compose pull || true
                        docker-compose up -d
                        sleep 30
                    '''
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    sh '''
                        for i in {1..10}; do
                            if curl -f http://localhost:8087/; then
                                echo "Application is healthy!"
                                exit 0
                            fi
                            sleep 10
                        done
                        echo "Health check failed!"
                        exit 1
                    '''
                }
            }
        }
    }
    
    post {
        success {
            echo '✅ Deployment successful!'
            echo 'Access at: http://localhost:8087'
        }
        failure {
            echo '❌ Deployment failed!'
            sh 'docker-compose logs --tail=50 || true'
        }
        always {
            sh 'docker image prune -f || true'
        }
    }
}
